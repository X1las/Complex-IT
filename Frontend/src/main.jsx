import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {BrowserRouter,Routes, Route} from 'react-router-dom'
import './index.css'
import App from './App.jsx'
import Profile from './routes/profile.jsx'
import Bookmarks from './routes/bookmarks.jsx'
import History from './routes/history.jsx'
import Ratings from './routes/ratings.jsx'
import Login from './routes/login.jsx'
import Register from './routes/register.jsx'
import Person from './routes/person.jsx'
import Title from './routes/title.jsx'
import Search from './routes/search.jsx'

const root = createRoot(document.getElementById('root'))
root.render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />}>
          
          <Route path="profile/:id/" element={<Profile/>}>
            <Route path="bookmarks" element={<Bookmarks/>} />
            <Route path="history" element={<History/>} />
            <Route path="ratings" element={<Ratings/>} />
          </Route>

          <Route path="login" element={<Login />}>
            <Route path="Register" element={<Register />} />
          </Route>
          
          <Route path="person/:id" element={<Person />} />
          <Route path="title/:id" element={<Title />} />
          <Route path="search" element={<Search />} />

          <Route path="*" element={<div>404 Not Found</div>} />

        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
