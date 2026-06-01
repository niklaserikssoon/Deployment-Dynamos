export function initSearch({ input, button, data, onResults }) {
  function runSearch() {
    const value = input.value.toLowerCase().trim();

    if (!value) {
      onResults(data);
      return;
    }

    const filtered = data.filter(exercise => {
      return (
        exercise.name.toLowerCase().includes(value) ||
        exercise.category?.toLowerCase().includes(value) ||
        exercise.level?.toLowerCase().includes(value) ||
        exercise.equipment?.toLowerCase().includes(value) ||
        exercise.primaryMuscles.some(m =>
          m.toLowerCase().includes(value)
        ) ||
        exercise.secondaryMuscles.some(m =>
          m.toLowerCase().includes(value)
        )
      );
    });

    onResults(filtered);
  }
  if(button)
    button.addEventListener("click", runSearch);

  if(input)
    input.addEventListener("keydown", e => {
      if (e.key === "Enter") runSearch();
    });
}