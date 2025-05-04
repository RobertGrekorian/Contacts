const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", (user, message) => {
    const msg = `${user}: ${message}`;
    const li = document.createElement('li');
    li.textContent = msg;
    document.getElementById("messageList").appendChild(li);
});

connection.start().catch(err => console.error(err.toString()))