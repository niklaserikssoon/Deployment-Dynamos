// ui/exercise-bank.js

// Renders the list of exercises in the UI

import { getExercises, deleteExercise } from '../storage/exercises.js'
// import { filterExercises, searchExercises } from '../logic/exerciseLogic.js'; Add when implementating these functions

/**
 * Renders all exercises in the list
 * Delete button for each of the exercises
 * exercisesUpdated is updated automatically when event is trigged
 */
export function renderExerciseBank() {
  const list = document.getElementById('exerciseList')
  if (!list) return

  // Clear current list
  list.style.minHeight = '240px'
  list.replaceChildren()

  // Fetch exercises
  const exerciseList = getExercises()
  const fragment = document.createDocumentFragment()

  // Render each exercise
  exerciseList.forEach((exercise) => {
    const li = document.createElement('li')
    li.textContent = exercise.name

    // Create delete button
    const deleteButton = document.createElement('button')
    deleteButton.textContent = 'Delete'

    // Delete event
    deleteButton.addEventListener('click', () => {
      deleteExercise(exercise.id)
    })

    // Add button to list item
    li.appendChild(deleteButton)

    // Add list item to UI
    fragment.appendChild(li)
  })

  // Append all items to the list at once
  list.appendChild(fragment)
}

export async function loadExercises() {
  const CACHE_KEY = 'cached_exercises'
  let allExercises = []

  try {
    const cached = localStorage.getItem(CACHE_KEY)

    if (cached) {
      allExercises = JSON.parse(cached)
    } else {
      const response = await fetch(
        'https://raw.githubusercontent.com/yuhonas/free-exercise-db/master/dist/exercises.json'
      )
      if (!response.ok)
        throw new Error(`Could not load exercises (${response.status})`)
      allExercises = await response.json()
      localStorage.setItem(CACHE_KEY, JSON.stringify(allExercises))
    }

    displayExercises(allExercises)
    return allExercises
  } catch (error) {
    console.error('loadExercises failed:', error)
    const gallery = document.getElementById('workout-display')
    if (gallery)
      gallery.innerHTML =
        '<p>Could not load exercises. Check your connection and try again.</p>'
    return []
  }
}

function displayExercises(exercises) {
  const gallery = document.getElementById('workout-display')
  if (!gallery) return
  gallery.innerHTML = ''

  const fragment = document.createDocumentFragment()

  exercises.forEach((exercise) => {
    const article = document.createElement('article')
    article.classList.add('card')
    article.innerHTML = `
    <h3>${exercise.name}</h3>
      <p><strong>Category:</strong> ${exercise.category}</p>
      <p><strong>Level:</strong> ${exercise.level}</p>
      <p><strong>Equipment:</strong> ${exercise.equipment ?? 'None'}</p>
      <p><strong>Primary muscles:</strong> ${exercise.primaryMuscles.join(', ')}</p>
      <p><strong>Secondary muscles:</strong> ${exercise.secondaryMuscles.join(', ')}</p>
    `
    fragment.appendChild(article)
  })

  gallery.appendChild(fragment)
}
