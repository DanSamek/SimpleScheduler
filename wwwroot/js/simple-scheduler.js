var connection = new signalR.HubConnectionBuilder().withUrl("/simple-scheduler-hub").build();
connection.start().then(function () {
    console.log("Starting simple-scheduler connection");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ExecutionUpdate", function (dto) {
    let execution = document.querySelector(`#job-${dto.id}`);
    if (execution == null){
        const parent = document.querySelector(".jobs tbody");
        const unknownJob = parent.querySelector("#job-unknown");
        execution = unknownJob.cloneNode(true);
        
        execution.style.removeProperty('display');
        execution.id = `job-${dto.id}`;
        parent.appendChild(execution);
    }

    execution.querySelector(".key").innerText = dto.key;
    const tag = execution.querySelector(".state").querySelector(".tag");
    tag.innerText = dto.state;
    tag.removeAttribute("class");
    tag.classList.add("tag");
    tag.classList.add(dto.state);
    execution.querySelector(".start-time").innerText = dto.started;
    execution.querySelector(".end-time").innerText = dto.ended;
    execution.querySelector(".detail").innerHTML =`<a href=\"/simple-scheduler/executions/${dto.id}\">Click</a></td>`
});

async function schedule(jobId){
    const result = await fetch(`/simple-scheduler/jobs/schedule/${jobId}`, {
        method: "POST"
    });
    console.log(result);
    // TODO alert (?)
    if (result.ok){
        
    }
    else{   
        
    }
}



document.addEventListener('DOMContentLoaded', () => {
    const navBarBurger = document.querySelector('.navbar-burger');
    
    navBarBurger.addEventListener('click', () => {
        document.querySelector('.navbar-menu').classList.toggle('is-active');
        navBarBurger.classList.toggle("is-active");
    })
    
});
