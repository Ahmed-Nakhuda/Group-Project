document.querySelectorAll('.product-checkbox').forEach((checkbox, index) => {
    checkbox.addEventListener('change', function () {
        const quantityInput = document.querySelectorAll('.quantity-input')[index];
        quantityInput.disabled = !this.checked;
        if (!this.checked) {
            quantityInput.value = ''; 
        }
    });
});
