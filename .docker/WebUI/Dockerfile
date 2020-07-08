FROM arriba-base as build

FROM node:10.21.0

COPY --from=build /src/Arriba/Arriba.Web/package.json /app/package.json
WORKDIR /app
RUN npm install

COPY --from=build /src/Arriba/Arriba.Web/ /app/

CMD ["npm", "run", "start"] 