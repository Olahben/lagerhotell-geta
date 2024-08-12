﻿global using System.Net.Http.Headers;
using LagerhotellAPI.Models.DomainModels.Validators;

namespace Lagerhotell.Services;

public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly SessionService _sessionService;
    private readonly string url = "https://localhost:7272/orders";
    private OrderValidator _orderValidator;

    public OrderService(HttpClient httpClient, SessionService sessionService)
    {
        _httpClient = httpClient;
        _sessionService = sessionService;
        _orderValidator = new OrderValidator();
    }

    public async Task<bool> ValidateOrder(Order order)
    {
        var validationResult = await _orderValidator.ValidateAsync(order);
        return validationResult.IsValid;
    }

    public async Task<string> CreateOrder(Order order)
    {
        string endpointUrl = url + "/add";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _sessionService.GetJwtFromLocalStorage());
        var newPendingOrder = order with { Status = OrderStatus.Pending };
        AddOrderRequest request = new AddOrderRequest { Order = newPendingOrder };
        string jsonData = JsonSerializer.Serialize(request);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync(endpointUrl, stringContent);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<AddOrderResponse>(responseContent, options).OrderId;
        }
        else
        {
            throw new InvalidOperationException($"Something went wrong when adding order, code: {response.StatusCode}");
        }
    }

    public async Task<Order> GetOrderByIdAsync(string id)
    {
        string endpointUrl = url + $"/{id}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _sessionService.GetJwtFromLocalStorage());
        HttpResponseMessage response = await _httpClient.GetAsync(endpointUrl);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Order>(responseContent, options);
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException("The order was not found");
        }
        else
        {
            throw new InvalidOperationException($"Something went wrong when fetching order, code: {response.StatusCode}");
        }
    }
}