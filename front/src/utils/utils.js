export const stringToColor = function (str) {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  var colour = "#";
  for (let j = 0; j < 3; j++) {
    let value = (hash >> (j * 8)) & 0xff;
    colour += ("00" + value.toString(16)).substr(-2);
  }
  return colour;
};

export const sortDates = (arr) => {
  return arr.sort(
    (firstDate, secondDate) => new Date(secondDate.creationDate) - new Date(firstDate.creationDate)
  );
};
