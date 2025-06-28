import axios from 'axios'
// import qs from 'qs'

const api = axios.create({
  baseURL: 'https://localhost:7258/api' // ← 改成你的 API 埠號
})

export const createStudent = (student) =>
  api.post('/students', student, {
    headers: {
      'Content-Type': 'application/json'
    }
  })
export const updateEnrollment = (id, data) =>
  api.put(`/enrollments/${id}`, data, {
    headers: { 'Content-Type': 'application/json' }
  })
export const getStudents = () => api.get('/students').then(res => res.data)
// export const createProduct = (product) => {
//  console.log('送出的資料:', product) // ← 在 Console 印出資料
//  return api.post('/products', product)
// }
// export const updateStudent = (id, student) => api.put(`/students/${id}`, student)
export const deleteStudent = (id) => api.delete(`/students/${id}`)
