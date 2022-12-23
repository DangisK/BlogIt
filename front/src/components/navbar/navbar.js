import {
  AppBar,
  Toolbar,
  styled,
  Typography,
  Box,
  Avatar,
  Menu,
  MenuItem,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Switch,
} from "@mui/material";
import React, { useContext } from "react";
import MenuIcon from "@mui/icons-material/Menu";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Home, Logout, ModeNight } from "@mui/icons-material";
import { stringToColor } from "../../utils/utils";
import { AppContext } from "../../context";

export const Navbar = ({ onLogout, mode, setMode }) => {
  const { user } = useContext(AppContext);
  const navigateTo = useNavigate();
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  const handleClick = (e) => {
    setAnchorEl(e.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  const [hamburgerAnchorEl, setHamburgerAnchorEl] = useState(null);
  const handleHamburgerClick = (event) => {
    setHamburgerAnchorEl(event.currentTarget);
  };

  const handleHamburgerClose = () => {
    setHamburgerAnchorEl(null);
  };

  return (
    <AppBar position="sticky">
      <StyledToolbar>
        <Typography
          variant="h6"
          sx={{ cursor: "pointer", display: { xs: "none", sm: "block" }, wordWrap: "break-word" }}
          onClick={() => navigateTo("/")}
        >
          Blog site
        </Typography>
        <>
          <IconButton
            color="inherit"
            aria-label="menu"
            onClick={handleHamburgerClick}
            sx={{ display: { xs: "block", sm: "none" } }}
          >
            <MenuIcon />
          </IconButton>
          <Menu
            anchorEl={hamburgerAnchorEl}
            keepMounted
            open={Boolean(hamburgerAnchorEl)}
            onClose={handleHamburgerClose}
            sx={{ display: { xs: "block", sm: "none" } }}
          >
            <List>
              <ListItem disablePadding>
                <ListItemButton component="a" href="/">
                  <ListItemIcon>
                    <Home />
                  </ListItemIcon>
                  <ListItemText primary="Homepage" />
                </ListItemButton>
              </ListItem>
              <ListItem disablePadding>
                <ListItemButton component="a" href="/auth" onClick={onLogout}>
                  <ListItemIcon>
                    <Logout />
                  </ListItemIcon>
                  <ListItemText primary="Logout" />
                </ListItemButton>
              </ListItem>
              <ListItem disablePadding>
                <ListItemButton component="a">
                  <ListItemIcon>
                    <ModeNight />
                  </ListItemIcon>
                  <Switch
                    defaultChecked
                    onChange={(e) => setMode(mode === "light" ? "dark" : "light")}
                  />
                </ListItemButton>
              </ListItem>
            </List>
          </Menu>
        </>
        <UserBox onClick={handleClick} sx={{ cursor: "pointer" }}>
          <Avatar sx={{ bgcolor: `${stringToColor(user.name.toUpperCase())}` }}>
            {user.name.charAt(0).toUpperCase()}
          </Avatar>
          {user.name}
        </UserBox>
      </StyledToolbar>
      <Menu
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
      >
        <MenuItem onClick={onLogout}>Logout</MenuItem>
      </Menu>
    </AppBar>
  );
};

const StyledToolbar = styled(Toolbar)({
  width: "100%",
  display: "flex",
  justifyContent: "space-between",
});

const Search = styled("div")(({ theme }) => ({
  backgroundColor: "white",
  padding: "0 10px",
  borderRadius: theme.shape.borderRadius,
  width: "40%",
}));

const Icons = styled(Box)(({ theme }) => ({
  display: "none",
  gap: "20px",
  alignItems: "center",
  [theme.breakpoints.up("sm")]: {
    display: "flex",
  },
}));

const UserBox = styled(Box)(({ theme }) => ({
  display: "flex",
  gap: "10px",
  alignItems: "center",
}));
