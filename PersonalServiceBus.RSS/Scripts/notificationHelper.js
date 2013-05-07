define([], function () {
    function showError(status, parentElement) {
        var hubWarningTemplate = '<div class="alert fade in"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Warning!</strong>&nbsp;<span class="alert-message">{ErrorMessage}</span></div>',
            hubErrorTemplate = '<div class="alert alert-error fade in"><button type="button" class="close" data-dismiss="alert">&times;</button><strong>Error!</strong>&nbsp;<span class="alert-error-message">{ErrorMessage}</span></div>',
            errorContainer;
        if ($(parentElement).length > 0) {
            errorContainer = $('.alert-container', parentElement);
        } else {
            errorContainer = $('.alert-container');
        }

        if (status.ErrorLevel > 3) {
            errorContainer.append(hubErrorTemplate.supplant(status));
        } else if (status.ErrorLevel == 3) {
            errorContainer.append(hubWarningTemplate.supplant(status));
        }
    }

    return {
        showError: showError
    };
});