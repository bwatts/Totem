using Microsoft.JSInterop;

namespace OutermindUI;

internal static class DreamJSInterop
{
    internal static ValueTask<bool> Alert(this IJSRuntime jsRuntime, string message) =>
        jsRuntime.InvokeAsync<bool>("DreamJSFunctions.alert", message);

    internal static ValueTask<bool> CapturePointer(this IJSRuntime jsRuntime, string elementId, long pointerId) =>
        jsRuntime.InvokeAsync<bool>("DreamJSFunctions.capturePointer", elementId, pointerId);

    internal static ValueTask<bool> ReleasePointer(this IJSRuntime jsRuntime, string elementId, long pointerId) =>
        jsRuntime.InvokeAsync<bool>("DreamJSFunctions.releasePointer", elementId, pointerId);
}
