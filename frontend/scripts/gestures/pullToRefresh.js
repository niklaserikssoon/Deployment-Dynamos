/**
 * Enable pull-to-refresh on a scrollable container
 * @param {HTMLElement} container - The container to pull
 * @param {Function} onRefresh - Async callback to run on refresh
 */
export function enablePullToRefresh(container, onRefresh) {
  if (!container) return

  let startY = 0
  let isPulling = false

  let indicator = container.querySelector(".pull-indicator")
  if (!indicator) {
    indicator = document.createElement("div")
    indicator.className = "pull-indicator"
    indicator.style.textAlign = "center"
    indicator.style.padding = "6px"
    indicator.style.fontSize = "0.9rem"
    indicator.textContent = "Pull down to refresh"
    container.prepend(indicator)
  }

  container.addEventListener("pointerdown", e => {
    if (container.scrollTop !== 0) return // only at top
    startY = e.clientY
  })

  container.addEventListener("pointermove", e => {
    const deltaY = e.clientY - startY
    if (deltaY > 50) {
      indicator.textContent = "Release to refresh..."
      isPulling = true
    }
  })

  container.addEventListener("pointerup", async () => {
    if (!isPulling) return
    indicator.textContent = "Refreshing..."
    await onRefresh()
    indicator.textContent = "Pull down to refresh"
    isPulling = false
  })
}
