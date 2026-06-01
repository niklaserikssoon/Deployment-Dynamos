import {
    getCurrentUser,
    getProfile,
    updateProfile,
    logout,

    addWeightProgress,
    addStrengthProgress,
    addCardioProgress,
    addAchievement,
    addTrainingSchedule,
    addTrainingGoal,
    addPersonalNote,
    updateStatistics,

    deleteTrainingGoal,
    deleteTrainingSchedule,
    deleteAchievement,
    deletePersonalNote,
    deleteWeightProgress,
    deleteStrengthProgress,
    deleteCardioProgress
} from "../storage/profileStorage.js";

document.addEventListener("DOMContentLoaded", async () => {

    const currentUser = getCurrentUser();
    if (!currentUser) {
        window.location.href = "login.html";
        return;
    }

    const refreshProfile = () => getProfile();
    let profile = refreshProfile();

    const profileForm = document.getElementById("profileForm");
    const profileMessage = document.getElementById("profileMessage");

    const goalsContainer = document.getElementById("trainingGoalsContainer");
    const progressContainer = document.getElementById("progressContainer");
    const statisticsContainer = document.getElementById("statisticsContainer");
    const scheduleContainer = document.getElementById("scheduleContainer");
    const achievementsContainer = document.getElementById("achievementsContainer");
    const notesContainer = document.getElementById("notesContainer");

    if (profileForm) {
        profileForm.name.value = profile.name;
        profileForm.age.value = profile.age;
        profileForm.gender.value = profile.gender;
        profileForm.weight.value = profile.weight;
        profileForm.height.value = profile.height;
        profileForm.experience.value = profile.experience;

        profileForm.addEventListener("submit", (e) => {
            e.preventDefault();
            profileMessage.textContent = "";

            const data = {
                name: profileForm.name.value.trim(),
                age: profileForm.age.value,
                gender: profileForm.gender.value,
                weight: profileForm.weight.value,
                height: profileForm.height.value,
                experience: profileForm.experience.value
            };

            updateProfile(data);
            profile = refreshProfile();
            profileMessage.textContent = "Profile Saved!";
        });
    }

    const isInSrc = location.pathname.includes('/src/');
    const indexPath = isInSrc ? '../index.html' : 'index.html';
    const loginPath = isInSrc ? 'login.html' : './src/login.html';

    document.getElementById("logoutButton")?.addEventListener("click", () => {
        logout();
        window.location.href = loginPath;
    });

    document.getElementById("goBackButton")?.addEventListener("click", () => {
        window.location.href = indexPath;
    });

    function renderGoals() {
        profile = refreshProfile();
        goalsContainer.innerHTML = "";

        profile.trainingGoals.forEach((goal, index) => {
            const article = document.createElement("article");
            article.innerHTML = `
                ${goal.title}: ${goal.progress}% (Target: ${goal.target}, Deadline: ${goal.deadline})
                <button data-index="${index}" class="delete-goal">Delete</button>
            `;
            goalsContainer.appendChild(article);
        });

        goalsContainer.querySelectorAll(".delete-goal").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteTrainingGoal(btn.dataset.index);
                renderGoals();
            });
        });
    }

    function renderProgress() {
        profile = refreshProfile();

        progressContainer.innerHTML = `
            <h3>Weight History</h3>
            <ul>
                ${profile.progress.weightHistory.map((w, i) =>
                    `<li>${w.date}: ${w.weight} kg 
                        <button data-index="${i}" class="delete-weight">X</button>
                    </li>`
                ).join("")}
            </ul>

            <h3>Strength History</h3>
            <ul>
                ${profile.progress.strengthHistory.map((s, i) =>
                    `<li>${s.date}: ${s.exercise} - ${s.value}
                        <button data-index="${i}" class="delete-strength">X</button>
                    </li>`
                ).join("")}
            </ul>

            <h3>Cardio History</h3>
            <ul>
                ${profile.progress.cardioHistory.map((c, i) =>
                    `<li>${c.date}: ${c.distance} km in ${c.time} min
                        <button data-index="${i}" class="delete-cardio">X</button>
                    </li>`
                ).join("")}
            </ul>
        `;

        progressContainer.querySelectorAll(".delete-weight").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteWeightProgress(btn.dataset.index);
                renderProgress();
            });
        });

        progressContainer.querySelectorAll(".delete-strength").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteStrengthProgress(btn.dataset.index);
                renderProgress();
            });
        });

        progressContainer.querySelectorAll(".delete-cardio").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteCardioProgress(btn.dataset.index);
                renderProgress();
            });
        });
    }

    async function renderStatistics() {
        await updateStatistics();
        profile = refreshProfile();

        const stats = profile.statistics;
        statisticsContainer.innerHTML = `
            <p>Total Sessions: ${stats.totalSessions}</p>
            <p>Total Minutes: ${stats.totalMinutes}</p>
            <p>Most Trained Muscle Group: ${stats.mostTrainedMuscleGroup}</p>
            <p>Average Sessions / Week: ${stats.averageSessionsPerWeek.toFixed(1)}</p>
        `;
    }

    function renderSchedule() {
        profile = refreshProfile();
        scheduleContainer.innerHTML = "";

        profile.schedule.forEach((item, index) => {
            const li = document.createElement("li");
            li.innerHTML = `
                ${item.date}: ${item.type} - ${item.notes}
                <button data-index="${index}" class="delete-schedule">Delete</button>
            `;
            scheduleContainer.appendChild(li);
        });

        scheduleContainer.querySelectorAll(".delete-schedule").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteTrainingSchedule(btn.dataset.index);
                renderSchedule();
            });
        });
    }

    function renderAchievements() {
        profile = refreshProfile();
        achievementsContainer.innerHTML = "";

        profile.achievements.forEach((ach, index) => {
            const li = document.createElement("li");
            li.innerHTML = `
                ${ach}
                <button data-index="${index}" class="delete-ach">Delete</button>
            `;
            achievementsContainer.appendChild(li);
        });

        achievementsContainer.querySelectorAll(".delete-ach").forEach(btn => {
            btn.addEventListener("click", () => {
                deleteAchievement(btn.dataset.index);
                renderAchievements();
            });
        });
    }

    function renderNotes() {
        profile = refreshProfile();
        notesContainer.innerHTML = "";

        profile.personalNotes.forEach((note, index) => {
            const li = document.createElement("li");
            li.innerHTML = `
                ${note.date}: ${note.text}
                <button data-index="${index}" class="delete-note">Delete</button>
            `;
            notesContainer.appendChild(li);
        });

        notesContainer.querySelectorAll(".delete-note").forEach(btn => {
            btn.addEventListener("click", () => {
                deletePersonalNote(btn.dataset.index);
                renderNotes();
            });
        });
    }

    renderGoals();
    renderProgress();
    renderStatistics();
    renderSchedule();
    renderAchievements();
    renderNotes();

    document.getElementById("addGoalForm")?.addEventListener("submit", (e) => {
        e.preventDefault();

        const title = e.target.goalTitle.value.trim();
        const target = e.target.goalTarget.value.trim();
        const deadline = e.target.goalDeadline.value;

        if (!title || !target || !deadline) return;

        addTrainingGoal(title, target, deadline);
        e.target.reset();
        renderGoals();
    });

    document.getElementById("addScheduleForm")?.addEventListener("submit", (e) => {
        e.preventDefault();

        const date = e.target.scheduleDate.value;
        const type = e.target.scheduleType.value;
        const notes = e.target.scheduleNotes.value.trim();

        if (!date || !type) return;

        addTrainingSchedule(date, type, notes);
        e.target.reset();
        renderSchedule();
    });

    document.getElementById("addAchievementForm")?.addEventListener("submit", (e) => {
        e.preventDefault();

        const text = e.target.achievementText.value.trim();
        if (!text) return;

        addAchievement(text);
        e.target.reset();
        renderAchievements();
    });

    document.getElementById("notesForm")?.addEventListener("submit", (e) => {
        e.preventDefault();

        const text = e.target.noteText.value.trim();
        if (!text) return;

        addPersonalNote(text);
        e.target.reset();
        renderNotes();
    });

    document.getElementById("addWeightForm")?.addEventListener("submit", (e) => {
        e.preventDefault();
        const weight = Number(e.target.weight.value);
        if (!weight) return;

        addWeightProgress(weight);
        e.target.reset();
        renderProgress();
    });

    document.getElementById("addStrengthForm")?.addEventListener("submit", (e) => {
        e.preventDefault();
        const exercise = e.target.strengthExercise.value.trim();
        const value = Number(e.target.strengthValue.value);
        if (!exercise || !value) return;

        addStrengthProgress(exercise, value);
        e.target.reset();
        renderProgress();
    });

    document.getElementById("addCardioForm")?.addEventListener("submit", (e) => {
        e.preventDefault();
        const distance = Number(e.target.cardioDistance.value);
        const time = Number(e.target.cardioTime.value);
        if (!distance || !time) return;

        addCardioProgress(distance, time);
        e.target.reset();
        renderProgress();
    });
});