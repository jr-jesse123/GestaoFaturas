$(document).ready(function () {
    // Configure jQuery Validation defaults
    $.validator.setDefaults({
        errorClass: 'is-invalid',
        validClass: 'is-valid',
        errorElement: 'span',
        errorPlacement: function (error, element) {
            if (element.parent('.form-check').length) {
                error.insertAfter(element.parent());
            } else {
                error.addClass('invalid-feedback');
                error.insertAfter(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass(errorClass).removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass(errorClass).addClass(validClass);
        }
    });

    // Custom validators
    $.validator.addMethod("phoneNumber", function(value, element) {
        if (!value) return true; // Optional field
        // Brazilian phone number format
        var phoneRegex = /^(\+55\s?)?(\([0-9]{2}\)\s?)?([0-9]{4,5}[-\s]?[0-9]{4})$/;
        return phoneRegex.test(value.replace(/\s/g, ''));
    }, "Please enter a valid phone number");

    $.validator.addMethod("uniqueEmail", function(value, element) {
        var isValid = false;
        var clientId = $('input[name="ResponsiblePerson.ClientId"]').val();
        var personId = $('input[name="ResponsiblePerson.Id"]').val() || 0;
        
        $.ajax({
            type: "GET",
            url: "/api/responsible-persons/check-email",
            data: {
                email: value,
                clientId: clientId,
                excludeId: personId
            },
            async: false,
            success: function(data) {
                isValid = !data.exists;
            }
        });
        
        return isValid;
    }, "This email is already registered for this client");

    // Apply validation to forms
    $('form').validate({
        rules: {
            'ResponsiblePerson.Name': {
                required: true,
                minlength: 2,
                maxlength: 100
            },
            'ResponsiblePerson.Email': {
                required: true,
                email: true,
                maxlength: 100,
                uniqueEmail: true
            },
            'ResponsiblePerson.Phone': {
                phoneNumber: true,
                maxlength: 20
            },
            'ResponsiblePerson.Role': {
                maxlength: 100
            },
            'ResponsiblePerson.Department': {
                maxlength: 50
            }
        },
        messages: {
            'ResponsiblePerson.Name': {
                required: "Name is required",
                minlength: "Name must be at least 2 characters",
                maxlength: "Name cannot exceed 100 characters"
            },
            'ResponsiblePerson.Email': {
                required: "Email is required",
                email: "Please enter a valid email address",
                maxlength: "Email cannot exceed 100 characters"
            },
            'ResponsiblePerson.Phone': {
                maxlength: "Phone cannot exceed 20 characters"
            },
            'ResponsiblePerson.Role': {
                maxlength: "Role cannot exceed 100 characters"
            },
            'ResponsiblePerson.Department': {
                maxlength: "Department cannot exceed 50 characters"
            }
        }
    });

    // Primary contact warning
    $('#isPrimaryContact').change(function() {
        if ($(this).is(':checked')) {
            var clientId = $('input[name="ResponsiblePerson.ClientId"]').val();
            var personId = $('input[name="ResponsiblePerson.Id"]').val() || 0;
            
            $.ajax({
                type: "GET",
                url: "/api/responsible-persons/check-primary",
                data: {
                    clientId: clientId,
                    excludeId: personId
                },
                success: function(data) {
                    if (data.hasPrimary) {
                        if (!confirm('This client already has a primary contact. Setting this person as primary will remove the primary status from the existing contact. Continue?')) {
                            $('#isPrimaryContact').prop('checked', false);
                        }
                    }
                }
            });
        }
    });

    // Active status warning
    $('#isActive').change(function() {
        if (!$(this).is(':checked')) {
            $('#receivesNotifications').prop('checked', false);
            $('#receivesNotifications').prop('disabled', true);
        } else {
            $('#receivesNotifications').prop('disabled', false);
        }
    });

    // Phone mask
    $('input[type="tel"]').on('input', function() {
        var value = $(this).val().replace(/\D/g, '');
        if (value.length > 0) {
            if (value.length <= 2) {
                value = '+' + value;
            } else if (value.length <= 4) {
                value = '+' + value.substring(0, 2) + ' ' + value.substring(2);
            } else if (value.length <= 6) {
                value = '+' + value.substring(0, 2) + ' (' + value.substring(2, 4) + ') ' + value.substring(4);
            } else if (value.length <= 10) {
                value = '+' + value.substring(0, 2) + ' (' + value.substring(2, 4) + ') ' + value.substring(4, 8) + '-' + value.substring(8);
            } else {
                value = '+' + value.substring(0, 2) + ' (' + value.substring(2, 4) + ') ' + value.substring(4, 9) + '-' + value.substring(9, 13);
            }
            $(this).val(value);
        }
    });
});