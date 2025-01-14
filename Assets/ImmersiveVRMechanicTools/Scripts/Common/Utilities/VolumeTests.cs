using UnityEngine;

public class VolumeTests
{
    public static bool IsPointInCylinder(Vector3 cylinderStart, Vector3 CylinderEnd, float radius, Vector3 testPoint)
    {
        var lenght = Vector3.Distance(cylinderStart, CylinderEnd);
        return GetDistanceSquaredFromCylinderAxis(cylinderStart, CylinderEnd, lenght * lenght, radius, testPoint) > 0;
    }
    
    private static float GetDistanceSquaredFromCylinderAxis(Vector3 cylinderStart, Vector3 CylinderEnd, float LengthSquared, float RadiusSquared, Vector3 testPoint)
    {
        float dx, dy, dz;	// vector d  from line segment point 1 to point 2
        float pdx, pdy, pdz;	// vector pd from point 1 to test point
        float dot, dsq;

        dx = CylinderEnd.x - cylinderStart.x;	// translate so pt1 is origin.  Make vector from
        dy = CylinderEnd.y - cylinderStart.y;     // pt1 to pt2.  Need for this is easily eliminated
        dz = CylinderEnd.z - cylinderStart.z;

        pdx = testPoint.x - cylinderStart.x;		// vector from pt1 to test point.
        pdy = testPoint.y - cylinderStart.y;
        pdz = testPoint.z - cylinderStart.z;

        // Dot the d and pd vectors to see if point lies behind the 
        // cylinder cap at pt1.x, pt1.y, pt1.z

        dot = pdx * dx + pdy * dy + pdz * dz;

        // If dot is less than zero the point is behind the pt1 cap.
        // If greater than the cylinder axis line segment length squared
        // then the point is outside the other end cap at pt2.

        if( dot < 0.0f || dot > LengthSquared )
        {
            return( -1.0f );
        }
        else 
        {
            // Point lies within the parallel caps, so find
            // distance squared from point to line, using the fact that sin^2 + cos^2 = 1
            // the dot = cos() * |d||pd|, and cross*cross = sin^2 * |d|^2 * |pd|^2
            // Carefull: '*' means mult for scalars and dotproduct for vectors
            // In short, where dist is pt distance to cyl axis: 
            // dist = sin( pd to d ) * |pd|
            // distsq = dsq = (1 - cos^2( pd to d)) * |pd|^2
            // dsq = ( 1 - (pd * d)^2 / (|pd|^2 * |d|^2) ) * |pd|^2
            // dsq = pd * pd - dot * dot / lengthsq
            //  where lengthsq is d*d or |d|^2 that is passed into this function 

            // distance squared to the cylinder axis:

            dsq = (pdx*pdx + pdy*pdy + pdz*pdz) - dot*dot/LengthSquared;

            if( dsq > RadiusSquared )
            {
                return( -1.0f );
            }
            else
            {
                return( dsq );		// return distance squared to axis
            }
        }
    }
}
