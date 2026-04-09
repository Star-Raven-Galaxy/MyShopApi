const baseUrl = "";


async function searchProduct() {
    const name = document.getElementById("searchInput").value;
    const response = await fetch(`${baseUrl}/products`);
    const products = await response.json();
    const product = products.find(p => p.name.toLowerCase() === name.toLowerCase());

    const resultDiv = document.getElementById("searchResult");
    if (product) {
        resultDiv.innerHTML = `
            <p>Найден товар: ${product.name} (${product.stock} шт.) - ${product.price} ₽</p>
            <button onclick="buyProduct(${product.id})">Купить</button>
        `;
    } else {
        resultDiv.innerHTML = "<p>Товар не найден</p>";
    }
}

async function buyProduct(productId) {
    // Получаем продукт
    let response = await fetch(`${baseUrl}/products/${productId}`);
    let product = await response.json();

    if (product.stock > 0) {
        product.stock -= 1;

        // Отправляем обновление на сервер
        await fetch(`${baseUrl}/products/${productId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(product)
        });

        alert(`Вы купили ${product.name}. Остаток: ${product.stock} шт.`);
        searchProduct(); // обновляем отображение
    } else {
        alert("Товара нет в наличии!");
    }
    getAllProducts();
}

async function getAllProducts() {
    const response = await fetch(`${baseUrl}/products`);
    const products = await response.json();

    const list = document.getElementById("productsList");
    list.innerHTML = "";

    products.forEach(p => {
        list.innerHTML += `
            <li>
                <b>${p.name}</b> - ${p.price} ₽  
                (Остаток: ${p.stock})
                <button onclick="buyProduct(${p.id})">Купить</button>
            </li>
        `;
    });
}
window.onload = function () {
    getAllProducts();
};