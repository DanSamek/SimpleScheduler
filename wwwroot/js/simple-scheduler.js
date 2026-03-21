var connection = new signalR.HubConnectionBuilder().withUrl("/simple-scheduler-hub").build();
connection.start().then(function () {
    console.log("Starting simple-scheduler connection");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("JobUpdate", function (dto) {
    const state = document.querySelector(`#job-${dto.id}`).querySelector(".state");
    state.innerHTML = dto.state;
});
