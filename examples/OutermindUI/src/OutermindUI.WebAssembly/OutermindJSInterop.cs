using Microsoft.JSInterop;

namespace OutermindUI;

internal static class OutermindJSInterop
{
    internal static ValueTask<bool> Alert(this IJSRuntime jsRuntime, string message) =>
        jsRuntime.InvokeAsync<bool>("OutermindJSFunctions.alert", message);

    internal static ValueTask<bool> CapturePointer(this IJSRuntime jsRuntime, string elementId, long pointerId) =>
        jsRuntime.InvokeAsync<bool>("OutermindJSFunctions.capturePointer", elementId, pointerId);

    internal static ValueTask<bool> ReleasePointer(this IJSRuntime jsRuntime, string elementId, long pointerId) =>
        jsRuntime.InvokeAsync<bool>("OutermindJSFunctions.releasePointer", elementId, pointerId);
}
