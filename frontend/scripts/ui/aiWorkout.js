const API_URL = `${CONFIG.workoutApiUrl}api/v1/ai/generate-plan`;


const btn = document.getElementById('ai-btn');

btn?.addEventListener('click', () => {
    const startSection = document.getElementById('start-workout');
    const heroPanel = startSection?.parentElement;

    if (document.getElementById('ai-section')) return;

    const section = document.createElement('section');
    section.id = 'ai-section';
    section.classList.add('card');
    section.innerHTML = `
        <h2 class="section-heading">AI Workout Plan</h2>
        <p class="section-subtitle">Tell us about your goals and we'll generate a plan for you.</p>

        <label for="ai-goal">Goal</label>
        <input type="text" id="ai-goal" placeholder="Ex: Build muscle, lose weight, improve cardio" />

        <label for="ai-level">Fitness Level</label>
        <select id="ai-level">
            <option value="Beginner">Beginner</option>
            <option value="Intermediate">Intermediate</option>
            <option value="Advanced">Advanced</option>
        </select>

        <label for="ai-days">Days per week</label>
        <input type="number" id="ai-days" min="1" max="7" value="3" />

        <label for="ai-equipment">Equipment (optional)</label>
        <input type="text" id="ai-equipment" placeholder="Ex: Dumbbells, barbell, none" />

        <button class="btn-accent" id="generate-plan-btn">Generate Plan</button>

        <div id="ai-result" hidden>
            <h3 class="subheading">Your Plan</h3>
            <pre id="ai-plan-output"></pre>
        </div>

        <p id="ai-error" hidden style="color: red;"></p>
        <button class="btn-secondary" id="ai-back-btn">⬅ Back</button>
    `;

    startSection.hidden = true;
    heroPanel.appendChild(section);

    document.getElementById('ai-back-btn').addEventListener('click', () => {
        section.remove();
        startSection.hidden = false;
    });

    document.getElementById('generate-plan-btn').addEventListener('click', async () => {
        const goal = document.getElementById('ai-goal').value.trim();
        const fitnessLevel = document.getElementById('ai-level').value;
        const daysPerWeek = Number(document.getElementById('ai-days').value);
        const equipment = document.getElementById('ai-equipment').value.trim();

        const generateBtn = document.getElementById('generate-plan-btn');
        const resultDiv = document.getElementById('ai-result');
        const planOutput = document.getElementById('ai-plan-output');
        const errorEl = document.getElementById('ai-error');

        if (!goal) {
            errorEl.textContent = 'Please enter a goal.';
            errorEl.hidden = false;
            return;
        }

        generateBtn.disabled = true;
        generateBtn.textContent = 'Generating...';
        resultDiv.hidden = true;
        errorEl.hidden = true;

        try {
            const token = localStorage.getItem('jwt_token');

            const res = await fetch(API_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ goal, fitnessLevel, daysPerWeek, equipment })
            });

            if (res.status === 401) throw new Error('You must be logged in.');
            if (res.status === 503) throw new Error('AI service timed out, try again.');
            if (!res.ok) throw new Error('Something went wrong, try again later.');

            const data = await res.json();
            planOutput.textContent = data.plan;
            resultDiv.hidden = false;
            resultDiv.scrollIntoView({ behavior: 'smooth' });

        } catch (err) {
            errorEl.textContent = err.message;
            errorEl.hidden = false;
        } finally {
            generateBtn.disabled = false;
            generateBtn.textContent = 'Generate Plan';
        }
    });
});