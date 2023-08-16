

$(document).ready(function () {

    //Add New Contact
    $("#btn-submit").click(function () {
        var formData = new FormData();
        formData.append("ContactName", $("#contactName").val());
        formData.append("ContactEmail", $("#contactEmail").val());
        formData.append("ContactAddress", $("#contactAddress").val());
        formData.append("ContactPhoneNo", $("#contactPhoneNo").val());
        formData.append("ContactProofFile", $("#contactProofFile")[0].files[0]);
        // Other properties

        $.ajax({
            type: "POST",
            url: "/Contacts/AddContact", 
            data: formData,
            contentType: false, // Set to false for FormData
            processData: false, // Set to false for FormData
            success: function (response) {
                console.log("Contact Created successfully");
                alert("Contact Created successfully");
                // Redirect to a success page or display a success message
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Error creating contact: " + errorThrown);
                alert("Error creating contact");
                // Display an error message to the user
            }
        });
    });


    //Upade Contact
    $("#btn-update").click(function () {
        var formData = new FormData();
        formData.append("ContactId", $("#upContactId").val());
        formData.append("ContactName", $("#upContactName").val());
        formData.append("ContactEmail", $("#upContactEmail").val());
        formData.append("ContactAddress", $("#upContactAddress").val());
        formData.append("ContactPhoneNo", $("#upContactPhoneNo").val());
        //formData.append("ContactProofFile", $("#editContactProofFile")[0].files[0]);

        // Create a new FormData instance for the file input
        var fileFormData = new FormData();
        var fileInput = $("#editContactProofFile")[0];
        if (fileInput.files.length > 0) {
            fileFormData.append("ContactProofFile", fileInput.files[0]);
        }

        // Combine the two FormData instances
        for (var key of formData.keys()) {
            fileFormData.append(key, formData.get(key));
        }

        $.ajax({
            type: "POST",
            url: "/Contacts/UpdateContact", 
            data: fileFormData,
            contentType: false, // Set to false for FormData
            processData: false, // Set to false for FormData
            success: function (response, textStatus) {
                console.log("Contact updated successfully");
                alert("Contact Updated successfully!!");
                window.location.href = "/Contacts/Index";
                // Redirect to a success page or display a success message
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Error updating contact: " + errorThrown);
                window.location.href = "/Contacts/Index";
                //alert("Error updating contact");
                // Display an error message to the user
            }
        });
    });


    // Live Search Functionality

    $("#searchInput").on("keyup", function () {
        var searchText = $(this).val();
        $.ajax({
            type: "GET",
            url: "/Contacts/SearchContacts", // Add a new action method for searching
            data: { searchText: searchText },
            success: function (data) {
                // Update the table content with filtered contacts
                $("#contactTable tbody").html(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Error searching contacts: " + errorThrown);
                // Handle error if needed
            }
        });
    });


    //VALIDATIONS 
    $("#ContactName").on("input", function () {
        var inputValue = $(this).val();
        if (!/^[a-zA-Z\s]*$/.test(inputValue)) {
            $(this).addClass("is-invalid");
            $("#spnContactName").text("Only alphabets are allowed!");
        } else {
            $(this).removeClass("is-invalid");
            $("#spnContactName").text("");
        }
    });

    // Validate Contact Email (Valid email format)
    $("#ContactEmail").on("input", function () {
        var inputValue = $(this).val();
        if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(inputValue)) {
            $(this).addClass("is-invalid");
            $("#spnContactEmail").text("Enter a valid email address!");
        } else {
            $(this).removeClass("is-invalid");
            $("#spnContactEmail").text("");
        }
    });

    // Validate Contact Phone Number (10 digits)
    $("#ContactPhoneNo").on("input", function () {
        var inputValue = $(this).val();
        if (!/^\d{10}$/.test(inputValue)) {
            $(this).addClass("is-invalid");
            $("#spnContactPhoneNo").text("Enter a valid PhoneNo!");
        } else {
            $(this).removeClass("is-invalid");
            $("#spnContactPhoneNo").text("");
        }
    });

    $("#ContactPhoneNo").on("input", function () {
        var maxLength = 10;
        var inputValue = $(this).val();

        // Remove non-digit characters from the input value
        var digitsOnly = inputValue.replace(/\D/g, '');

        // Limit the input value to maxLength digits
        var trimmedValue = digitsOnly.substring(0, maxLength);

        $(this).val(trimmedValue);
    });

    // Validate Contact Proof File (Size)
    $("#ContactProofFile").on("change", function () {
        var fileSize = this.files[0].size;
        if (fileSize > 10 * 1024 * 1024) {
            $(this).addClass("is-invalid");
            $("#spnContactProof").text("File size should no be greater than 10Mb!");
        } else {
            $(this).removeClass("is-invalid");
            $("#spnContactProof").text("");
        }
    });

    // Form submission
    $("#btn-submit").click(function () {
        var formIsValid = true;

        // Check if any field is invalid
        if ($(".is-invalid").length > 0) {
            formIsValid = false;
            // Optionally show a validation message here
        }

        if (formIsValid) {
            // Proceed with form submission
            $("#createContactForm").submit();
        } else {
            // Display an error message or prevent submission
        }
    });


    $("#editContactProofFile").on("change", function () {
        var existingFileName = $(this).data("existing-filename");
        var selectedFileName = $(this).val().split("\\").pop(); // Get the selected file name
        $("#ExistingFileNameSpan").text(selectedFileName || existingFileName);
    });



});