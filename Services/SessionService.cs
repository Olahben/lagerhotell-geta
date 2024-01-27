﻿using Microsoft.JSInterop;

namespace Lagerhotell.Services.UserService
{
    public class SessionService
    {
        private readonly string _baseUrl = "https://localhost:7272/users";
        private readonly IJSRuntime _jsRuntime;
        public SessionService(IJSRuntime jsRuntime)
        {
            this._jsRuntime = jsRuntime;
        }
        public async Task? AddJwtToLocalStorage(string jwt)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", jwt);
        }

        public async Task<string> GetJwtFromLocalStorage()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
            return token ?? null;
        }

        /*public async Task<string> GetTokenPhoneNumber(string token)
        {
            DecodeJwt.DecodeJwtRequest request = new() { Token = token };
            string url = _baseUrl + "/decode-token";
            return "";
        } */

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

    }
}