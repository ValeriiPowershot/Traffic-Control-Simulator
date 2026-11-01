using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    public static class CityCarFactory
    {
        private static Material tyre, glass;
        public static Material Material(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader); material.color = color; return material;
        }
        public static GameObject Box(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool solid = false)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale;
            go.AddComponent<MeshFilter>().sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            go.AddComponent<MeshRenderer>().sharedMaterial = material;
            if (solid) go.AddComponent<BoxCollider>();
            return go;
        }
        public static CityCarController Create(CityRoadPath road, float distance, int origin, Color color, Transform parent = null)
        {
            if (!tyre) tyre = Material(new Color(0.06f, 0.07f, 0.08f));
            if (!glass) glass = Material(new Color(0.16f, 0.28f, 0.35f));
            var go = new GameObject("City Car " + origin); go.SetActive(false); go.transform.SetParent(parent, false);
            var sample = road.At(distance);
            go.transform.rotation = Quaternion.LookRotation(sample.Tangent);
            go.transform.position = sample.Position + Vector3.up * 0.7f + sample.Tangent * 1.2f;
            var body = go.AddComponent<Rigidbody>(); body.mass = 1100; body.drag = 0.025f; body.angularDrag = 0.6f;
            body.centerOfMass = new Vector3(0, -0.25f, 0);
            var car = go.AddComponent<CityCarController>();
            car.Chassis = go.AddComponent<BoxCollider>(); car.Chassis.size = new Vector3(1.55f, 0.65f, 3.6f);
            var paint = Material(color);
            Box("Body visual", go.transform, Vector3.zero, car.Chassis.size, paint);
            Box("Cabin", go.transform, new Vector3(0, 0.45f, -0.1f), new Vector3(1.35f, 0.55f, 1.75f), glass);
            var wheels = new WheelCollider[4]; car.WheelModels = new Transform[4];
            for (int i = 0; i < 4; i++)
            {
                var axle = new GameObject(i < 2 ? "Front wheel" : "Rear wheel"); axle.transform.SetParent(go.transform, false);
                axle.transform.localPosition = new Vector3(i % 2 == 0 ? -0.72f : 0.72f, -0.2f, i < 2 ? 1.2f : -1.2f);
                var w = axle.AddComponent<WheelCollider>(); w.mass = 18; w.radius = 0.31f; w.suspensionDistance = 0.22f;
                var spring = w.suspensionSpring; spring.spring = 32000; spring.damper = 4200; spring.targetPosition = 0.5f; w.suspensionSpring = spring;
                var forward = w.forwardFriction; forward.stiffness = 1.2f; w.forwardFriction = forward;
                var sideways = w.sidewaysFriction; sideways.stiffness = 1.3f; w.sidewaysFriction = sideways;
                var model = new GameObject("Wheel visual"); model.transform.SetParent(go.transform, false);
                var mesh = new GameObject("Tyre"); mesh.transform.SetParent(model.transform, false);
                mesh.transform.localRotation = Quaternion.Euler(0, 0, 90); mesh.transform.localScale = new Vector3(0.62f, 0.12f, 0.62f);
                mesh.AddComponent<MeshFilter>().sharedMesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
                mesh.AddComponent<MeshRenderer>().sharedMaterial = tyre;
                car.WheelModels[i] = model.transform; wheels[i] = w;
            }
            car.FrontLeft = wheels[0]; car.FrontRight = wheels[1]; car.RearLeft = wheels[2]; car.RearRight = wheels[3];
            car.Road = road; go.SetActive(true); car.Prepare(road, origin);
            // Each generated body owns its paint material.
            go.AddComponent<CityCarPaint>().OwnedMaterial = paint;
            return car;
        }
    }
}
