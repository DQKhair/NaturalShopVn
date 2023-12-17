$(document).ready(() => {

    AddToCart();

    function AddToCart() {
        const currentURL = window.location.href;
        const segments = currentURL.split('/');

        const productId = segments[segments.length - 1];
       
        $("#addToCart").click(() => {

            var quantity = document.getElementById('inputQuantity').value;

            const dataJson = {
                "productId": productId,
                "quantity": quantity
            }   

             $.ajax({
                 type: "POST",
                 url: `/AddToCart/productId=${productId}&&quantity=${quantity}`,
                 dataType: "json",
                 contentType: "application/json",
                 data: JSON.stringify(dataJson), 
                 success: (res) => {
                     console.log(res.value)
                     alert("Thêm vào giỏ hàng thành công");
                     location.reload();
                 },
                 error: (error) => {
                     console.error("Error add to cart", error);
                 }
             })
        })
    }
})