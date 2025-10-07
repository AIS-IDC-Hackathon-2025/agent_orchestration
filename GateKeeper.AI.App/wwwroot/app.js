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