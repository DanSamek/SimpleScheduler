var connection = new signalR.HubConnectionBuilder().withUrl("/simple-scheduler-hub").build();
connection.start().then(function () {
    console.log("Starting simple-scheduler connection");
}).catch(function (err) {
    return console.error(err.toString());
});


// TODO move for only pages where its needed.
connection.on("ExecutionUpdate", function (dto) {
    console.log(dto);
    let currentExecution = document.querySelector(`.exec-${dto.id}`)
    if (currentExecution == null) {
        return;
    }
    
    let progressBar = document.querySelector(`.progress-bar`);
    if (dto.state !== "Running") {
        progressBar.classList.add("is-hidden");
    } else {
        progressBar.classList.remove("is-hidden");
    }
    
    let info = document.querySelector(".update-info");
    info.innerHTML = `
        <p>State: ${dto.state}</p>
        <p>Started: ${dto.started}</p>
        <p>Ended: ${dto.ended}</p> 
    `;
});

async function schedule(jobId, arguments){
    const result = await fetch(`/simple-scheduler/jobs/schedule`, {
        method: "POST",
        body: JSON.stringify({'arguments' : arguments, 'id' : jobId }),
        headers: {
            "Content-type": 'application/json'
        }
    });
    
    if (result.ok) {
        
    }
    else {
        
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const navBarBurger = document.querySelector('.navbar-burger');
    
    navBarBurger.addEventListener('click', () => {
        document.querySelector('.navbar-menu').classList.toggle('is-active');
        navBarBurger.classList.toggle("is-active");
    })
    
});
