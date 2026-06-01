// generateWorkout.js copie for Node.js testing environment (since fetch isn't available in Jest by default)

const API_URL =
  "https://raw.githubusercontent.com/yuhonas/free-exercise-db/master/dist/exercises.json";

let cachedExercises = [];

async function loadExercises() {
  if (cachedExercises.length) return cachedExercises;

  const res = await fetch(API_URL);
  cachedExercises = await res.json();
  return cachedExercises;
}

function pickRandomUnique(arr, count) {
  const shuffled = [...arr].sort(() => 0.5 - Math.random());
  return shuffled.slice(0, Math.min(count, arr.length));
}

function randomSetsReps() {
  const sets = Math.floor(Math.random() * 3) + 3;
  const reps = Math.floor(Math.random() * 8) + 8;
  return { sets, reps };
}

async function generateWorkout(muscle, count = 5) {
  const exercises = await loadExercises();

  const filtered = exercises.filter(ex =>
    ex.primaryMuscles.map(m => m.toLowerCase()).includes(muscle.toLowerCase())
  );

  const selected = pickRandomUnique(filtered, count);

  return selected.map(ex => ({
    ...ex,
    ...randomSetsReps(),
  }));
}

module.exports = { generateWorkout, loadExercises };
