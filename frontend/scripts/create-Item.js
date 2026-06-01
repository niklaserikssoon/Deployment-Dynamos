// scripts/create-Item.js
// Handles Create form submission and saves a new exercise

import { addExercise } from './storage/exercises.js'

export function initCreateForm() {
  const form = document.getElementById('create-item-form')
  if (!form) return

  form.addEventListener('submit', (e) => {
    e.preventDefault()

    const category = document.getElementById('category')?.value?.trim() ?? ''
    const muscleGroup =
      document.getElementById('muscleGroup')?.value?.trim() ?? ''
    const equipment = document.getElementById('equipment')?.value?.trim() ?? ''
    const level = document.getElementById('level')?.value?.trim() ?? ''

    // ✅ Step 2: minimal validation (required fields)
    if (!category) {
      alert('Please select a category.')
      return
    }
    if (!muscleGroup) {
      alert('Please enter a muscle group.')
      return
    }
    if (!equipment) {
      alert('Please select equipment.')
      return
    }
    if (!level) {
      alert('Please select level.')
      return
    }

    const newExercise = {
      id: Date.now(), // simple unique id
      name: `${category} - ${muscleGroup}`, // temporary name for now
      equipment,
      level,
    }

    addExercise(newExercise)
    form.reset()

    // Success feedback (Step 4)
    const feedback = document.getElementById('create-feedback') // UI message area
    if (feedback) {
      feedback.textContent = '✅ Exercise saved!'
      feedback.hidden = false

      // Auto-hide after a short moment
      setTimeout(() => {
        feedback.hidden = true
        feedback.textContent = ''
      }, 2000)
    }
  })
}
