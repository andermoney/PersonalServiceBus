define([], function () {
    function getFormData($elem) {
        var data = {};
        $('input', $elem).each(function () {
            var name = $(this).attr('name');
            if (name) {
                data[name] = $(this).val();
            }
        });
        return data;
    }

    function clearForm($elem) {
        $('input', $elem).each(function () {
            var name = $(this).attr('name');
            if (name) {
                $(this).val('');
            }
        });
    }

    function selectFirstField($elem) {
        $('input:first[name]', $elem).focus();
    }

    return {
        getFormData: getFormData,
        clearForm: clearForm,
        selectFirstField: selectFirstField
    };
});