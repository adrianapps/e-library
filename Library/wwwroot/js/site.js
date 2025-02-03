// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(document).ready(function () {
//    $(".add-to-cart").click(function () {
//        var bookId = $(this).data("id");

//        $.ajax({
//            url: "/Cart/AddToCart",
//            type: "POST",
//            data: { bookId: bookId },
//            success: function (response) {
//                if (response.success) {
//                    alert("✅ Książka została dodana do koszyka!");
//                } else {
//                    alert("⚠ Nie udało się dodać książki do koszyka: " + response.message);
//                }
//            },
//            error: function () {
//                alert("Błąd podczas dodawania do koszyka.");
//            }
//        });
//    });
//});

//$(document).ready(function () {
//    let timeout;

//    function performSearch() {
//        clearTimeout(timeout);
//        var query = $('#searchQuery').val();
//        var categoryId = $('#category').val();

//        timeout = setTimeout(function () {
//            $.ajax({
//                url: '@Url.Action("Search2", "Book")',
//                type: 'GET',
//                data: { query: query, categoryId: categoryId },
//                success: function (data) {
//                    $('#searchResults').html(data);
//                },
//                error: function () {
//                    alert('Wystąpił błąd.');
//                }
//            });
//        }, 300); // 300 ms opóźnienia
//    }

//    // Obsługa wpisywania w pole wyszukiwania
//    $('#searchQuery').on('input', function () {
//        performSearch();
//    });

//    // Obsługa zmiany kategorii
//    $('#category').on('change', function () {
//        performSearch();
//    });
//});
