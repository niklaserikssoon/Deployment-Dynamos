document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.getElementById('theme-toggle')

  if (localStorage.getItem('theme') === 'dark') {
    document.documentElement.classList.add('dark')
  }

  if (toggle) {
    toggle.addEventListener('click', () => {
      document.documentElement.classList.toggle('dark')
      const isDark = document.documentElement.classList.contains('dark')
      localStorage.setItem('theme', isDark ? 'dark' : 'light')
      toggle.setAttribute('aria-pressed', String(isDark))
    })
  }
})
