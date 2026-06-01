import { 
    login, 
    registerUser, 
    seedUsers, 
    getProfile, 
    isProfileEmpty 
} from "../storage/profileStorage.js";

document.addEventListener("DOMContentLoaded", async () => {
    await seedUsers();

    const tabRegister = document.getElementById("tab-register");
    const tabLogin = document.getElementById("tab-login");

    const panelRegister = document.getElementById("panel-register");
    const panelLogin = document.getElementById("panel-login");

    function activateTab(name) {
        const isRegister = name === "register";
        if (tabRegister) tabRegister.setAttribute("aria-selected", isRegister ? "true" : "false");
        if (tabLogin) tabLogin.setAttribute("aria-selected", isRegister ? "false" : "true");

        if (panelRegister) panelRegister.hidden = !isRegister;
        if (panelLogin) panelLogin.hidden = isRegister;

        history.replaceState(null, "", `#${name}`);
    }

    function switchTab(tab) {
        if (tab === "register") activateTab("register");
        else activateTab("login");
    }

    if (tabRegister) tabRegister.addEventListener("click", () => switchTab("register"));
    if (tabLogin) tabLogin.addEventListener("click", () => switchTab("login"));

    [tabRegister, tabLogin].filter(Boolean).forEach((t) => {
        t.addEventListener("keydown", (e) => {
            if (e.key === "ArrowRight" || e.key === "ArrowLeft") {
                const target = e.key === "ArrowRight" ? "login" : "register";
                switchTab(target);
            }
        });
    });

    const hash = (location.hash || "").replace("#", "");
    if (hash === "register" || hash === "login") {
        activateTab(hash);
    } else {
        const initial = document.querySelector('[role="tab"][aria-selected="true"]');
        if (initial && initial.id === "tab-register") activateTab("register");
        else activateTab("login");
    }

    const isInSrc = location.pathname.includes('/src/');
    const paths = {
      index: isInSrc ? '../index.html' : 'index.html',
      profile: isInSrc ? 'profile.html' : './src/profile.html'
    };

    const loginForm = document.getElementById("loginForm");
    const registerForm = document.getElementById("registerForm");

    const loginMessage = document.getElementById("loginMessage");
    const registerMessage = document.getElementById("registerMessage");

    // LOGIN
    if (loginForm) {
        loginForm.addEventListener("submit", (e) => {
            e.preventDefault();
            if (loginMessage) loginMessage.textContent = "";

            const name = loginForm.name?.value?.trim() || "";
            const password = loginForm.password?.value || "";

            try {
                login(name, password);

                const profile = getProfile();

                if (isProfileEmpty(profile)) {
                    window.location.href = paths.profile;
                } else {
                    window.location.href = paths.index;
                }

            } catch (err) {
                if (loginMessage) loginMessage.textContent = err.message || String(err);
                else console.error(err);
            }
        });
    }

    // REGISTER
    if (registerForm) {
        registerForm.addEventListener("submit", (e) => {
            e.preventDefault();
            if (registerMessage) registerMessage.textContent = "";

            const name = registerForm.name?.value?.trim() || "";
            const password = registerForm.password?.value || "";
            const email = registerForm.email?.value?.trim() || "";

            try {
                registerUser(name, password, email);

                login(name, password);

                const profile = getProfile();
                if (isProfileEmpty(profile)) {
                    window.location.href = paths.profile;
                } else {
                    window.location.href = paths.index;
                }

            } catch (err) {
                if (registerMessage) registerMessage.textContent = err.message || String(err);
                else console.error(err);
            }
        });
    }
});