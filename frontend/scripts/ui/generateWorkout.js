// generate random workout from API exercise database
const API_URL =
  'https://raw.githubusercontent.com/yuhonas/free-exercise-db/master/dist/exercises.json'

let cachedExercises = []

export async function loadExercises() {
  if (cachedExercises.length) return cachedExercises;
  try {
    const res = await fetch(API_URL)
    if (!res.ok) throw new Error(`Could not load exercises (${res.status})`)
    cachedExercises = await res.json()
    return cachedExercises
  } catch (error) {
    console.error('generateWorkout loadExercises failed:', error)
    return []
  }
}

function pickRandomUnique(arr, count) {
  const shuffled = [...arr].sort(() => 0.5 - Math.random())
  return shuffled.slice(0, Math.min(count, arr.length))
}

function randomSetsReps() {
  const sets = Math.floor(Math.random() * 3) + 3
  const reps = Math.floor(Math.random() * 8) + 8
  return { sets, reps }
}

export async function generateWorkout(muscle, count = 5) {
  const exercises = await loadExercises()

  const filtered = exercises.filter((ex) =>
    ex.primaryMuscles.map((m) => m.toLowerCase()).includes(muscle.toLowerCase())
  )

  const selected = pickRandomUnique(filtered, count)

  return selected.map((ex) => ({
    ...ex,
    ...randomSetsReps(),
  }))
}
