// ui/index.js

// Renders the list of items in the UI

import { getItems } from '../storage/exercises.js'

export function renderList() {
  const container = document.getElementById('item-list')
  if (!container) return

  const items = getItems()

  container.style.minHeight = '200px'

  container.replaceChildren()

  if (items.length === 0) {
    const empty = document.createElement('p')
    empty.className = 'empty-state'
    empty.textContent = 'No items yet. Add your first workout!'
    container.appendChild(empty)
    return
  }

  const fragment = document.createDocumentFragment()

  items.forEach((item) => {
    const li = document.createElement('li')
    li.textContent = item.name
    fragment.appendChild(li)
  })

  container.appendChild(fragment)
}

export function initWorkoutNav() {
  const startSection = document.getElementById('start-workout')
  const optionsSection = document.getElementById('workout-options')
  const generateSection = document.getElementById('generate-workout')
  const customSection = document.getElementById('custom-workout')

  if (!startSection || !optionsSection || !generateSection || !customSection)
    return

  document.getElementById('start-btn').addEventListener('click', () => {
    startSection.hidden = true
    optionsSection.hidden = false
  })

  document.getElementById('generate-option').addEventListener('click', () => {
    optionsSection.hidden = true
    generateSection.hidden = false
  })

  document.getElementById('custom-option').addEventListener('click', () => {
    optionsSection.hidden = true
    customSection.hidden = false
  })

  document.getElementById('options-back-btn').addEventListener('click', () => {
    optionsSection.hidden = true
    startSection.hidden = false
  })

  document.getElementById('generate-back-btn').addEventListener('click', () => {
    generateSection.hidden = true
    optionsSection.hidden = false
  })

  document.getElementById('custom-back-btn').addEventListener('click', () => {
    customSection.hidden = true
    optionsSection.hidden = false
  })
}
