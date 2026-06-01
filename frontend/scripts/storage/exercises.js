// storage/exercises.js

// Handles all data management for exercises (CRUD operations)

/**
 * Fetches all exercises from LocalStorage
 * Returns an empty array if nothing is saved
 */
export function getExercises() {
  return JSON.parse(localStorage.getItem('exercises')) || []
}

/**
 * Adds a new exercise to LocalStorage
 * Saves the updated list and notifies the UI
 */
export function addExercise(newExercise) {
  const exerciseList = getExercises()

  exerciseList.push(newExercise)

  localStorage.setItem('exercises', JSON.stringify(exerciseList))

  // Notify UI
  document.dispatchEvent(new Event('exercisesUpdated'))

  return newExercise
}

/**
 * Deletes an exercise from LocalStorage based on its ID
 * Shows a confirmation dialog before deleting
 * Saves the updated list in LocalStorage
 * Dispatches a global event so the UI can update automatically
 */
export function deleteExercise(idToDelete) {
  const confirmDelete = confirm(
    'Are you sure you want to delete this exercise?'
  )
  if (!confirmDelete) return getExercises() // Return current list if cancelled

  const exerciseList = getExercises()

  const updatedExerciseList = exerciseList.filter((ex) => ex.id !== idToDelete)

  // Save updated list
  localStorage.setItem('exercises', JSON.stringify(updatedExerciseList))

  // Notify UI
  document.dispatchEvent(new Event('exercisesUpdated'))

  return updatedExerciseList
}

// Moved from itemsStorage.js
export function getItems() {
  const items = JSON.parse(localStorage.getItem('items')) || []
  return items.sort((a, b) => b.createdAt - a.createdAt) // newest first
}
