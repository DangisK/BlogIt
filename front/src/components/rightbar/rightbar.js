import {
  Avatar,
  AvatarGroup,
  Box,
  Divider,
  ImageList,
  ImageListItem,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Typography,
} from "@mui/material";
import React, { useContext } from "react";
import { AppContext } from "../../context";
import { stringToColor } from "../../utils/utils";

export const Rightbar = () => {
  const { user } = useContext(AppContext);
  return (
    <Box flex={1.5} p={2} sx={{ display: { xs: "none", lg: "block" } }}>
      <Box position="fixed" maxWidth={300}>
        <Typography variant="h6" fontWeight={100} sx={{ wordWrap: "break-word" }}>
          You are logged in as:
        </Typography>
        <List sx={{ width: "100%", maxWidth: 360, bgcolor: "background.paper" }}>
          <ListItem alignItems="flex-start">
            <ListItemAvatar sx={{ marginTop: 0 }}>
              <Avatar sx={{ bgcolor: `${stringToColor(user.name.toUpperCase())}` }}>
                {user.name.charAt(0).toUpperCase()}
              </Avatar>
            </ListItemAvatar>
            <ListItemText
              primary={
                <Typography variant="h5" sx={{ wordWrap: "break-word" }}>
                  {user.name}
                </Typography>
              }
              secondary={
                <React.Fragment>
                  <Typography
                    sx={{ display: "inline" }}
                    component="span"
                    variant="body2"
                    color="text.primary"
                  >
                    Roles:
                  </Typography>
                  <Typography variant="body2" sx={{ wordWrap: "break-word" }}>
                    {user.roles.join(" ")}
                  </Typography>
                </React.Fragment>
              }
            />
          </ListItem>
        </List>
        <Divider />
      </Box>
    </Box>
  );
};
