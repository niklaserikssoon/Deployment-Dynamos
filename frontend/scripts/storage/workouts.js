const workoutService = (function () {
  const STORAGE_KEY = "workouts";

  function getWorkouts() {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    } catch {
      console.warn("Invalid workout data in storage. Resetting.");
      localStorage.removeItem(STORAGE_KEY);
      return [];
    }
  }

  function saveWorkouts(workouts) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(workouts));
  }

  function addWorkout(workout) {
    if (!workout.id) {
      workout.id =
        typeof crypto !== "undefined" && crypto.randomUUID
          ? crypto.randomUUID()
          : Date.now();
    }

    const workouts = getWorkouts();
    saveWorkouts([workout, ...workouts]);
  }

  function updateWorkout(updatedWorkout) {
    const workouts = getWorkouts().map(w =>
      w.id === updatedWorkout.id ? updatedWorkout : w
    );
    saveWorkouts(workouts);
  }

  function getWorkoutById(id) {
      return getWorkouts().find(w => w.id === id);
    }

    function deleteWorkout(id) {
    const workouts = getWorkouts().filter(w => w.id !== id);
    saveWorkouts(workouts);
  }

  function createWorkout() {
    return {
      date: new Date().toISOString().split("T")[0],
      exercises: [
        {
          exerciseId: 1,
          sets: 5,
          reps: 10
        }
      ]
    };
  }

  return {
    getWorkouts,
    saveWorkouts,
    addWorkout,
    updateWorkout,
    getWorkoutById,
    deleteWorkout,
    createWorkout
  };
})();

export default workoutService;
