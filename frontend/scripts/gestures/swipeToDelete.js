/**
 * Enable swipe-to-delete on a given element
 * @param {HTMLElement} element - The item to swipe
 * @param {Function} onDelete - Callback when swipe passes threshold
 */
export function enableSwipeToDelete(element, onDelete) {
  let startX = 0
  let currentX = 0
  let isDragging = false

  if (!element) return

  element.tabIndex = 0 // make keyboard focusable

  // Pointer (touch/mouse) events
  element.addEventListener("pointerdown", e => {
    startX = e.clientX
    isDragging = true
    element.setPointerCapture(e.pointerId)
  })

  element.addEventListener("pointermove", e => {
    if (!isDragging) return
    currentX = e.clientX
    const delta = Math.min(0, currentX - startX)
    element.style.transform = `translateX(${delta}px)`
  })

  element.addEventListener("pointerup", () => {
    isDragging = false
    const delta = currentX - startX
    element.style.transform = "translateX(0)"
    if (delta < -100) onDelete()
  })

  // Keyboard fallback (accessible)
  element.addEventListener("keydown", e => {
    if (e.key === "Delete" || e.key === "Backspace") {
      onDelete()
    }
  })
}
