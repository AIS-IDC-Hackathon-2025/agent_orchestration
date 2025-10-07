let API_BASE_URL = "https://localhost:7160";

window.initializeSignalRConnection = (dotNetHelper) => {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl(`${API_BASE_URL}/agentshub`,
            {
                //transport: signalR.HttpTransportType.WebSockets,
                //skipNegotiation: true
            })
        .build();

    connection.on("ReceiveMessage", (message) => {
        const lines = message.split("\n");
        for (const payload of lines) {
            dotNetHelper.invokeMethodAsync('ChartCompletetionsStreamJs', payload);
        }
    });

    connection.start().catch((err) => {
        return console.error(err.toString());
    });
}

// Add this new function for scrolling to bottom
window.scrollElementToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}