namespace MyShopApi.Delegates;

public delegate void OnRequestCompleted(string endpoint, int statusCode, long elapsedMs);