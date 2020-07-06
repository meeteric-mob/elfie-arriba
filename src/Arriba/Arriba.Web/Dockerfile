FROM node:8.17.0-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .
COPY configuration.default/ configuration/

RUN npm run build

EXPOSE 8080

CMD ["npm", "start"]