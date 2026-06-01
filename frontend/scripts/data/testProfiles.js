function loadDebugData() {
    const users = JSON.parse(localStorage.getItem("users")) || [];
    const currentUser = localStorage.getItem("currentUser") || null;

    users.sort((a, b) => a.name.localeCompare(b.name));

    const formatted = users.map(u => ({
        name: u.name,
        email: u.email,
        password: u.password,
        profile: u.profile
    }));

    return JSON.stringify(
        {
            currentUser,
            users: formatted
        },
        null,
        2
    );
}

document.getElementById("output").textContent = loadDebugData();