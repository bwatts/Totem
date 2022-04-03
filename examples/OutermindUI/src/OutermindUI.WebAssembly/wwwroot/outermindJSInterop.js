window.OutermindJSFunctions = {
    alert: function(message) {
        alert(message);
        return true;
    },
    capturePointer: function (elementId, pointerId) {
        const element = document.getElementById(elementId)

        if (element !== null) {
            element.setPointerCapture(pointerId);
            return true;
        }

        return false;
    },
    releasePointer: function (elementId, pointerId) {
        const element = document.getElementById(elementId)

        if (element !== null) {
            element.releasePointerCapture(pointerId);
            return true;
        }

        return false;
    }
}
