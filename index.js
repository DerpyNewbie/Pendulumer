console.log('Happy developing ✨')


async function loadLeaderboard() {
    let response = await fetch("https://assets-1.derpynewbie.dev/api/v1/leaderboard");
    response.json().then(records => {
        let html = "";
        for (let i = 0; i < records.length; i++) {
            let record = records[i];
            html = "<td><td>" + record["rank"] + "</td><td>" + record["name"] + "</td><td>" + record["score"] + "m</td></tr>";
            document.getElementById("leaderboard").innerHTML = html;
        }
    })
}

loadLeaderboard().then(leaderboard => {console.log("Loaded leaderboard" + leaderboard)});