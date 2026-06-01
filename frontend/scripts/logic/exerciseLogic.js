// logic/exerciseLogic.js

// Pure logic that does not depend on UI or LocalStorage

/* search and filter items function */
export function filterExercises(exercises, searchTerm, category){
    return exercises.filter(exercise => {
        const matchesSearch = exercise.name.toLowerCase().includes(searchTerm.toLowerCase());

        const matchesCategory = 
            category === 'all' || exercise.category === category;
            
            return matchesSearch && matchesCategory;
    });
}
export const searchExercises = filterExercises;