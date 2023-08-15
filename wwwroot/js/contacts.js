

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
                console.log("Contact added successfully");
                alert("Contact added successfully");
                // Redirect to a success page or display a success message
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Error adding contact: " + errorThrown);
                alert("Error adding contact");
                // Display an error message to the user
            }
        });
    });


    //Upade Contact
    $("#btn-update").click(function () {
        var formData = new FormData();
        formData.append("ContactId", $("#contactId").val());
        formData.append("ContactName", $("#contactName").val());
        formData.append("ContactEmail", $("#contactEmail").val());
        formData.append("ContactAddress", $("#contactAddress").val());
        formData.append("ContactPhoneNo", $("#contactPhoneNo").val());
        //formData.append("ContactProofFile", $("#contactProofFile")[0].files[0]);
        // Other properties

        $.ajax({
            type: "POST",
            url: "/Contacts/UpdateContact", // Update with the correct URL
            data: formData,
            contentType: false, // Set to false for FormData
            processData: false, // Set to false for FormData
            success: function (response, textStatus) {
                console.log("Contact updated successfully");
                alert("Contact updated successfully");
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

    

});