// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function selectRow(row) {
    // Get all rows in the table body
    const rows = document.querySelectorAll("#ticketsTable tbody tr");

    // Remove the "table-active" class from all rows (clear selection)
    rows.forEach(r => r.classList.remove("table-active"));

    // Add the "table-active" class to the clicked row (select it)
    row.classList.add("table-active");

    // Store the selected row's data-ticket-id in the hidden field
    const SN = row.getAttribute("data-ticket-id");
    
    document.getElementById("selectedSN").value = SN;
    
    console.log("Selected Ticket ID:", SN); // Debugging log
}
function fsa(){
        alert("برای درج پیوست از صفحه لیست تیکت ابتدا تیکت مورد نظر را انتخاب کنید.");
}
function selectJobOpportunity(row) {
    const rows = document.querySelectorAll("#JobOpportunityTable tbody tr");

    // Remove the "table-active" class from all rows (clear selection)
    rows.forEach(r => r.classList.remove("table-active"));

    // Add the "table-active" class to the clicked row (select it)
    row.classList.add("table-active");

    // Get all columns (td) in the selected row
    const cells = row.querySelectorAll("td");
    const SN = row.getAttribute("data-SN");
    //console.log("سلام:", cells[0].textContent, cells[1].textContent);
    document.getElementById("NationalID").value = cells[0].textContent; // Assuming SN is the first column
    document.getElementById("FirstName").value = cells[1].textContent;
    document.getElementById("LastName").value = cells[2].textContent;
    document.getElementById("CityDs").value = cells[3].textContent;
    document.getElementById("Mobile").value = cells[4].textContent;
    document.getElementById("Email").value = cells[5].textContent;
    document.getElementById("selectedSN").value = SN;

}
function emptyAllFields(row)
{
    const inputs = document.querySelectorAll('#input-fields input');

    // Loop through all inputs and clear their values
    inputs.forEach(input => {
        // Check if the input type is file (to clear file input)
        if (input.type === 'file') {
            input.value = '';  // Clear file input value
        } else {
            input.value = '';  // Clear other inputs (text, number, etc.)
        }
    });

    // Optionally, clear any other fields like selects, textareas, etc.
    const textareas = document.querySelectorAll('#input-fields textarea');
    textareas.forEach(textarea => {
        textarea.value = ''; // Clear textarea values if any
    });

    // Prevent form submission to allow clearing before submitting
    event.preventDefault();
}

//function saveSelection() {
//    const selectedTicketId = document.getElementById("selectedTicketId").value;
//    if (selectedTicketId) {
//        alert(`Selected Ticket ID: ${selectedTicketId}`);
//        // Perform additional actions, such as sending the data to the server
//    } else {
//        alert("No row selected!");
//    }
//}
document.addEventListener('DOMContentLoaded', function () {
    const rows = document.querySelectorAll('table tbody tr[asp-page]');

    rows.forEach(row => {
        row.addEventListener('dblclick', function () {
            const url = row.getAttribute('asp-page');
            const id = row.getAttribute('asp-route-id');
            if (url && id) {
                window.location.href = `${url}/${id}`; // Navigate to the Razor Page
            }
            console.log(url, id);
        });
    });
});
function disableButton(button) {
    //const button = document.getElementById(btn);
    
    if (button) {
        button.disabled = true; // Disable the button
        button.classList.add('disabled');
    } else {
        console.error(`Button with ID "${button}" not found.`);
    }
    //button.addEventListener('click', function (event) {
    //    event.preventDefault();
    //});
    console.log(`"${button}"`);
}
function enableButton(btn) {
    const button = document.getElementById(btn);
    if (button) {
        button.disabled = false; // Disable the button
        button.classList.remove('disabled');
    } else {
        console.error(`Button with ID "${button}" not found.`);
    }
    //button.addEventListener('click', function (event) {
    //    event.preventDefault();
    //});
    console.log(`"${button}"`);
}
function enableAllBtn() {

}