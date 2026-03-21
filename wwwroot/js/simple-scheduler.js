var connection = new signalR.HubConnectionBuilder().withUrl("/simple-scheduler-hub").build();
connection.start().then(function () {
    console.log("Starting simple-scheduler connection");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ExecutionUpdate", function (dto) {
    console.log(dto);
    let execution = document.querySelector(`#job-${dto.id}`);
    if (execution == null){
        
        const parent = document.querySelector(".jobs");
        const unknownJob = parent.querySelector("#job-unknown");
        const executionClone = unknownJob.cloneNode(true);
        
        executionClone.style.removeProperty('display');
        executionClone.id = `job-${dto.id}`;
        executionClone.querySelector(".key").innerText = dto.key; 
        executionClone.querySelector(".state").innerText = dto.state;
        executionClone.querySelector(".execution-time").innerText = dto.executionTime;
        parent.appendChild(executionClone);
        return;
    }
    
    const state = execution.querySelector(".state");
    state.innerHTML = dto.state;
});

/*
*     <div style="display: none" id="job-unknown">
        <span class="key">0</span>
        <span class="state">0</span>
        <span class="execution-time">0</span>
    </div>
* */