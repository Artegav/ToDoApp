// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function hideCompletedItems(checked) {
    var tableRows = document.querySelectorAll('.table tbody tr');
    tableRows.forEach(function (row) {
        if (row.classList.contains('completed')) {
            row.style.display = checked ? 'none' : '';
        }
    });
}
