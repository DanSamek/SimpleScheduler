var connection = new signalR.HubConnectionBuilder().withUrl("/simple-scheduler-hub").build();
connection.start().then(function () {
    console.log("Starting simple-scheduler connection");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ExecutionUpdate", function (dto) {
    const execution = document.querySelector(`#job-${dto.id}`);
    if (execution == null){
        
        const parent = document.querySelector(".jobs");
        const unknownJob = parent.querySelector("#job-unknown");
        const executionClone = unknownJob.cloneNode(true);
        
        executionClone.style.removeProperty('display');
        executionClone.id = `job-${dto.id}`;
        executionClone.querySelector(".key").innerText = dto.key; 
        executionClone.querySelector(".state").innerText = dto.state;
        executionClone.querySelector(".start-time").innerText = dto.started;
        executionClone.querySelector(".end-time").innerText = dto.ended;
        parent.appendChild(executionClone);
        return;
    }
    
    execution.querySelector(".state").innerText = dto.state;
    execution.querySelector(".end-time").innerText = dto.ended;
});