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
    console.log(dto);
    info.innerHTML = `
        <p>State: ${dto.state}</p>
        <p>Started: ${dto.started}</p>
        <p>Ended: ${dto.ended}</p> 
        <p>Retry count:  ${dto.retryCount}</p> 
    `;
    
    
    console.log(dto);
    let currentValue = window.editor.getValue();

    if (dto.error !== null) {
        currentValue += `\n// Got an error/errors:\n\n/* ${dto.error} */`;
    }
    
    window.editor.setValue(currentValue);
});

document.addEventListener('DOMContentLoaded', () => {
    const navBarBurger = document.querySelector('.navbar-burger');
    
    navBarBurger.addEventListener('click', () => {
        document.querySelector('.navbar-menu').classList.toggle('is-active');
        navBarBurger.classList.toggle("is-active");
    })
    
});
