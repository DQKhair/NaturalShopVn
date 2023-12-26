$(document).ready(() => {

    AddToCart();

    function AddToCart() {
        const currentURL = window.location.href;
        const segments = currentURL.split('/');

        const productId = segments[segments.length - 1];
       
        $("#addToCart").click(() => {

            var quantity = document.getElementById('inputQuantity').value;
            var productQuantity = document.getElementById('productQuantity').innerText;
            const dataJson = {
                "productId": productId,
                "quantity": quantity
            }   
            if (productQuantity > 0 && productQuantity > quantity) {
                $.ajax({
                    type: "POST",
                    url: `/AddToCart/productId=${productId}&&quantity=${quantity}`,
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(dataJson),
                    success: (res) => {
                        console.log(res.value)
                        //alert("Sản phẩm đã được thêm vào giỏ hàng");
                        location.reload();
                    },
                    error: (error) => {
                        console.error("Error add to cart", error);
                    }
                })
            } else {
                alert('Sản phẩm không đủ số lượng');
            }
             
        })
    }
})