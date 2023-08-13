$(document).ready(function () {
    $("#getEmployeeButton").click(function () {
        var empId = $("#employeeId").val();

        // Make an AJAX request to the API endpoint
        $.ajax({
            type: "GET",
            url: "/api/Employee/GetEmployeeDetail/" + empId,
            success: function (data) {
                // Update the employee details on success
                $("#empId").text(data.employeeId);
                $("#empName").text(data.employeeName);
                $("#empLocation").text(data.employeeLocation);
                $("#empPhoneNo").text(data.employeePhoneNo);
                // Update other employee details here
            },
            error: function () {
                alert("Failed to retrieve employee details.");
            }
        });
    });
});
