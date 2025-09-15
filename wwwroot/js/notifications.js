const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("ReceiveNotification", function (message, url) {
    console.log("📢 إشعار:", message, url);
    // هنا تقدر تحدث HTML أو تعمل Toast
    let container = document.getElementById("notifications");
    if (container) {
        let item = document.createElement("li");
        item.innerHTML = `<a href="${url || '#'}">${message}</a>`;
        container.prepend(item);
    }
});

connection.start().catch(err => console.error(err.toString()));
