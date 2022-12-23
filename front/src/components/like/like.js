import { Favorite, ThumbDown, ThumbUp } from "@mui/icons-material";
import { Checkbox, IconButton, styled } from "@mui/material";
import styles from "./styles.module.css";

export const Like = ({ votes, handleUpvote, handleDownvote, value }) => {
  return (
    <div className={styles.like}>
      <IconButton onClick={handleUpvote}>
        <StyledCheckBox
          icon={<ThumbUp />}
          checked={value === 1}
          checkedIcon={<ThumbUp sx={{ color: "#FFD700" }} />}
        />
      </IconButton>
      {votes}
      <IconButton onClick={handleDownvote}>
        <StyledCheckBox
          icon={<ThumbDown />}
          checked={value === -1}
          checkedIcon={<ThumbDown sx={{ color: "#FFD700" }} />}
        />
      </IconButton>
    </div>
  );
};

const StyledCheckBox = styled(Checkbox)({
  display: "flex",
  alignItems: "flex-start",
});
