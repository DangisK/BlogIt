import { IconButton } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import styles from "./styles.module.css";

export const NavigationBar = () => {
  return (
    <header>
      <a href="">Home</a>
      <div className={styles.hamburger}>
        <IconButton edge="start" color="blue" aria-label="menu" sx={{ mr: 2 }}>
          <MenuIcon />
        </IconButton>
      </div>
      <nav className={styles.navbar}>
        <ul>
          <li>
            <a href="">Pirmassss</a>
          </li>
          <li>
            <a href="">Antraas</a>
            <ul className={styles.sublinks}>
              <li>
                <a href="">Pirmas</a>
              </li>
              <li>
                <a href="">Antras</a>
              </li>
              <li>
                <a href="">Trecias</a>
              </li>
            </ul>
          </li>
          <li>
            <a href="">Trecias</a>
            <ul className={styles.sublinks}>
              <li>
                <a href="">Pirmas</a>
              </li>
              <li>
                <a href="">Antras</a>
              </li>
              <li>
                <a href="">Trecias</a>
              </li>
              <li>
                <a href="">Ketvirtas</a>
              </li>
              <li>
                <a href="">Penktas</a>
              </li>
            </ul>
          </li>
          <li>
            <a href="">Ketvirtas</a>
          </li>
        </ul>
      </nav>
    </header>
  );
};
