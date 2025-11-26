import { useState } from 'react'
import {BrowserRouter, Routes} from 'react-router-dom'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div>
        <p>Hello, Welcome to Complex-IT Application Frontend!</p>
    </div>
  )
}

export default App
